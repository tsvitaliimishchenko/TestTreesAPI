using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using TestTreesAPI.Models;
using Microsoft.EntityFrameworkCore;
using TestTreesAPI.DataAccess;
using TestTreesAPI.Services;
using TestTreesAPI.Exceptions;

namespace TestTreesAPI.Controllers
{
    [ApiController]
    [Route("api.tree")]
    [Produces("application/json")]
    public class TreeController : ControllerBase
    {
        private readonly TreeDbContext _treeDbContext;
        private readonly DbLoggerService _dbLoggerService;

        public TreeController(TreeDbContext treeDbContext, ExceptionLogDbContext exceptionLogDbContext)
        {
            _treeDbContext = treeDbContext;
            _dbLoggerService = new DbLoggerService(exceptionLogDbContext);
        }

        [HttpPost]
        [Route("get")]
        public async Task<ActionResult> Get(string treeName)
        {
            try
            {
                var rootNode = _treeDbContext.Nodes.FirstOrDefault(n => n.ParentNodeId == null && n.Name == treeName);

                if(rootNode == null)
                {
                    var tree = new Node(treeName, treeName, null, true);

                    _treeDbContext.Nodes.Add(tree);
                    await _treeDbContext.SaveChangesAsync();
                    return Ok(tree);
                }

                return Ok(GetTreeWithChildren(rootNode.Id));

            }
            catch (Exception ex)
            {
                var eventId = Guid.NewGuid();
                _dbLoggerService.LogException(ex, Request, eventId);
                return StatusCode(500, new { type = ex.GetType().Name, id = eventId.ToString(), data = new { message = $"Internal server error ID = {eventId}" } });
            }
        }

        [HttpPost]
        [Route("node.create")]
        public async Task<ActionResult> Create(string treeName, int parentNodeId, string nodeName)
        {
            try
            {
                var parentNode = _treeDbContext.Nodes.Include(x => x.Children).FirstOrDefault(x => x.Id == parentNodeId);
                if (parentNode == null)
                    throw new SecureException("Parent node wasn't found");
                if (parentNode.TreeName != treeName)
                    throw new SecureException("Requested parent node was found, but it doesn't belong your tree");
                if (parentNode != null && parentNode.Children.Any(n => n.Name == nodeName))
                    throw new SecureException("Duplicate name");

                var node = new Node(nodeName, treeName, parentNodeId, false);

                _treeDbContext.Nodes.Add(node);
                await _treeDbContext.SaveChangesAsync();
                return Ok();
            }
            catch (SecureException ex)
            {
                _dbLoggerService.LogException(ex, Request, ex.EventId);
                return StatusCode(500, new { type = ex.GetType().Name, id = ex.EventId.ToString(), data = new { message = ex.Message } });
            }
            catch (Exception ex)
            {
                var eventId = Guid.NewGuid();
                _dbLoggerService.LogException(ex, Request, eventId);
                return StatusCode(500, new { type = ex.GetType().Name, id = eventId.ToString(), data = new { message = $"Internal server error ID = {eventId}" } });
            }
        }

        [HttpPost]
        [Route("node.delete")]
        public async Task<ActionResult> Delete(string treeName, int nodeId)
        {
            try
            {
                var node = await _treeDbContext.Nodes.Include(n => n.Children).FirstOrDefaultAsync(n => n.Id == nodeId);

                if (node == null)
                    throw new SecureException("Node wasn't found");
                if (node.TreeName != treeName)
                    throw new SecureException("Requested node was found, but it doesn't belong your tree");
                if(node.Children.Any())
                    throw new SecureException("Can't delete node with leaves");

                _treeDbContext.Nodes.Remove(node);
                await _treeDbContext.SaveChangesAsync();
                return Ok();
            }
            catch (SecureException ex)
            {
                _dbLoggerService.LogException(ex, Request, ex.EventId);
                return StatusCode(500, new { type = ex.GetType().Name, id = ex.EventId.ToString(), data = new { message = ex.Message } });
            }
            catch (Exception ex)
            {
                var eventId = Guid.NewGuid();
                _dbLoggerService.LogException(ex, Request, eventId);
                return StatusCode(500, new { type = ex.GetType().Name, id = eventId.ToString(), data = new { message = $"Internal server error ID = {eventId}" } });
            }
        }

        [HttpPost]
        [Route("node.rename")]
        public async Task<ActionResult> Rename(string treeName, int nodeId, string newNodeName)
        {
            try
            {
                var node = await _treeDbContext.Nodes.FirstOrDefaultAsync(n => n.Id == nodeId);

                var parentNode = await _treeDbContext.Nodes.Include(x => x.Children).FirstOrDefaultAsync(n => n.Id == node.ParentNodeId);
                if (node == null)
                    throw new SecureException("Node wasn't found");
                if (node.TreeName != treeName)
                    throw new SecureException("Requested node was found, but it doesn't belong your tree");
                if (parentNode != null && parentNode.Children.Any(n => n.Name == newNodeName))
                    throw new SecureException("Duplicate name");

                node.Name = newNodeName;
                await _treeDbContext.SaveChangesAsync();

                return Ok();
            }
            catch (SecureException ex)
            {
                _dbLoggerService.LogException(ex, Request, ex.EventId);
                return StatusCode(500, new { type = ex.GetType().Name, id = ex.EventId.ToString(), data = new { message = ex.Message } });
            }
            catch (Exception ex)
            {
                var eventId = Guid.NewGuid();
                _dbLoggerService.LogException(ex, Request, eventId);
                return StatusCode(500, new { type = ex.GetType().Name, id = eventId.ToString(), data = new { message = $"Internal server error ID = {eventId}" } });
            }
        }

        private Node GetTreeWithChildren(int id)
        {
            var tree = _treeDbContext.Nodes.Include(t => t.Children).FirstOrDefault(t => t.Id == id);

            if (tree != null)
            {
                foreach (var child in tree.Children.ToList())
                {
                    GetTreeWithChildren(child.Id);
                }
            }

            return tree;
        }
    }
}
